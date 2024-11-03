using Docker.DotNet;
using Docker.DotNet.Models;
using DockerBackup.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DockerBackup.App.Jobs;

public class DockerVolumeBackupJob : IJob
{
    private readonly IDockerClient _dockerClient;
    private readonly ILogger<DockerVolumeBackupJob> _logger;
    private readonly BackupsDbContext _db;
    private readonly IConfiguration _configs;

    public DockerVolumeBackupJob(IDockerClient dockerClient, ILogger<DockerVolumeBackupJob> logger, BackupsDbContext db, IConfiguration configs)
    {
        _dockerClient = dockerClient;
        _logger = logger;
        _db = db;
        _configs = configs;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        string s3Key = _configs["DOCKERBACKUP_S3_KEY"];
        string s3Secret = _configs["DOCKERBACKUP_S3_SECRET"];
        string s3Endpoint = _configs["DOCKERBACKUP_S3_ENDPOINT"];

        if (string.IsNullOrWhiteSpace(s3Key) || string.IsNullOrWhiteSpace(s3Secret) ||
            string.IsNullOrWhiteSpace(s3Endpoint))
        {
            _logger.LogError("S3 configurations environment variables not found, exiting job");

            return;
        }

        string container = context.MergedJobDataMap["container"] as string;

        _logger.LogInformation($"Backup Job Started for container '{container}'...");

        _logger.LogInformation("Loading volumes to backup...");
        var volumes = _db.ContainerVolumes.Where(x => x.Schedule.ContainerName == container).ToList();
        _logger.LogInformation("Volumes loaded.");

        _logger.LogInformation("Pulling rclone:latest image...");
        await _dockerClient.Images.CreateImageAsync(new ImagesCreateParameters()
        {
            FromImage = "rclone/rclone:latest",
            Tag = "latest"
        }, null, new Progress<JSONMessage>());
        _logger.LogInformation("Pull complete.");

        foreach (var volume in volumes)
        {
            _logger.LogInformation($"Backing up volume '{volume.ContainerPath}'...");

            var containerResponse = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                Image = "rclone/rclone:latest",
                Env = new List<string>()
                {
                    "RCLONE_S3_PROVIDER=Other",
                    "RCLONE_S3_ENV_AUTH=true",
                    $"RCLONE_S3_ACCESS_KEY_ID={s3Key}",
                    $"RCLONE_S3_SECRET_ACCESS_KEY={s3Secret}",
                    $"RCLONE_S3_ENDPOINT={s3Endpoint}"
                },
                HostConfig = new HostConfig()
                {
                    Mounts = new List<Mount>()
                    {
                        new Mount()
                        {
                            Source = volume.HostPath,
                            Target = "/data",
                            ReadOnly = true,
                            Type = "bind"
                        }
                    }
                },
                Entrypoint = new List<string>() { "/bin/sh", "-c", $"(apk add zip && rclone config create s3 s3 env_auth=true && cd /data && (zip -r - . | rclone rcat s3:jess-raspi-backups/{DateTime.Now.ToString("yyyy-M-d")}/{container.TrimStart('/')}_{volume.ContainerPath.Replace("/","_")}.zip))" },
            });
            var containerStarted = await _dockerClient.Containers.StartContainerAsync(containerResponse.ID, new ContainerStartParameters());

            var containerWaitResponse = await _dockerClient.Containers.WaitContainerAsync(containerResponse.ID);

            if (containerWaitResponse.StatusCode != 0)
            {
                _logger.LogError($"Backup for container '{container}' failed. {containerWaitResponse?.Error?.Message}");
            }

            await _dockerClient.Containers.RemoveContainerAsync(containerResponse.ID, new ContainerRemoveParameters()
            {
                Force = true,
                RemoveVolumes = false
            });

            _logger.LogInformation($"Backup complete.");

        }

        _logger.LogInformation($"Backup Job for container '{container}' Finished.");
    }
}