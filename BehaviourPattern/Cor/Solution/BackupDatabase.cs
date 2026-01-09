using System.Diagnostics;

namespace DesignPattern.BehaviourPattern.Cor.Solution;

/*
 * Build backup with Decorator 
 */

public class BackupDatabaseRequest
{
    public required string SourcePath { get; set; }
    public byte[] Data { get; set; } = [];
    public long OriginalSize { get; set; }
    public long CompressedSize { get; set; }
    public bool IsCompressed { get; set; }
    public bool IsEncrypted { get; set; }
    public string Checksum { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class DatabaseCollectionHandle : Handler<BackupDatabaseRequest>
{
    public override HandlerResult Process(Request<BackupDatabaseRequest> request)
    {
        var data = request.Data;
        Console.WriteLine($"[{Name}] Collecting data from: {data.SourcePath}");

        // Giả lập đọc dữ liệu
        Thread.Sleep(300);
        data.Data         = new byte[1024 * 1024 * 10]; // 10MB
        data.OriginalSize = data.Data.Length;
        data.Timestamp    = DateTime.Now;

        Console.WriteLine($"[{Name}] ✓ Collected {data.OriginalSize / 1024 / 1024} MB");
        return HandlerResult.Continue;
    }
}

public class CloudUpHandle : Handler<BackupDatabaseRequest>
{
    public override HandlerResult Process(Request<BackupDatabaseRequest> request)
    {
        var data = request.Data;
        Console.WriteLine($"[{Name}] Collecting data from: {data.SourcePath}");

        Thread.Sleep(5000);

        var sizeToUpload = data.IsCompressed ? data.CompressedSize : data.OriginalSize;
        Console.WriteLine($"[{Name}] ✓ Uploaded {sizeToUpload / 1024 / 1024} MB"); return HandlerResult.Continue; } }

public class VerificationHandler : Handler<BackupDatabaseRequest>
{
    public override HandlerResult Process(Request<BackupDatabaseRequest> request)
    {
        var data = request.Data;
        Console.WriteLine($"[{Name}] Verifying backup integrity...");
        Thread.Sleep(200);
        Console.WriteLine($"[{Name}] ✓ Checksum: {data.Checksum}");
        return HandlerResult.Handled;
    }
}

// =============================================================================
// DECORATOR - Thêm tính năng cho handlers 
// =============================================================================

public class BackupHandlerDecorator(Handler<BackupDatabaseRequest> wrapper) : Handler<BackupDatabaseRequest>
{
    public override HandlerResult Process(Request<BackupDatabaseRequest> request)
    {
        return wrapper.Process(request);
    }
}

public class CompressionDecorator(Handler<BackupDatabaseRequest> handler) : BackupHandlerDecorator(handler)
{
    public override HandlerResult Process(Request<BackupDatabaseRequest> request)
    {
        var data = request.Data;
        if (data.IsCompressed) return base.Process(request);

        Console.WriteLine($"[CompressionDecorator] Compressing data ... ");
        Thread.Sleep(300);
        data.CompressedSize = (long)(data.CompressedSize * 0.6);
        data.IsCompressed   = true;
        Console.WriteLine($"[CompressionDecorator] Compression successfully ");

        return base.Process(request);
    }
}

public class EncryptionDecoration(Handler<BackupDatabaseRequest> handler, string encryptionKey = "AES256Key")
    : BackupHandlerDecorator(handler)
{
    public override HandlerResult Process(Request<BackupDatabaseRequest> request)
    {
        var data = request.Data;
        Console.WriteLine($"\n╭─ [{GetType().Name}] Encrypting with AES-256...");
        Thread.Sleep(300);
        data.IsEncrypted = true;
        Console.WriteLine($"│  Algorithm: AES-256");
        Console.WriteLine($"│  Key: {encryptionKey[..8]}***");
        Console.WriteLine($"╰─ ✓ Encryption completed\n");

        return base.Process(request);
    }
}

public class PerformanceMonitoringDecorator(Handler<BackupDatabaseRequest> wrapper) : BackupHandlerDecorator(wrapper)
{
    private static readonly Dictionary<string, List<long>> metrics;

    public override HandlerResult Process(Request<BackupDatabaseRequest> request)
    {
        var stopwatch = Stopwatch.StartNew();

        Console.WriteLine($"\n⏱ [{GetType().Name}] Starting performance monitoring...");

        var result = base.Process(request);

        stopwatch.Stop();
        var elapsed = stopwatch.ElapsedMilliseconds;

        Console.WriteLine($"⏱ [{GetType().Name}] Execution time: {elapsed} ms");

        return result;
    }
}


//
// // CoR
// var collectChain = new DatabaseCollectionHandle();
// var uploadChain  = new CloudUpHandle();
// var verifyChain  = new VerificationHandler();
//
// // Decorate
// Handler<BackupDatabaseRequest> collectionDecorate = collectChain;
// collectionDecorate = new CompressionDecorator(collectionDecorate);
// collectionDecorate = new EncryptionDecoration(collectionDecorate);
//
// Handler<BackupDatabaseRequest> verifyDecorate = verifyChain;
// verifyDecorate = new PerformanceMonitoringDecorator(verifyDecorate);
//
// var request = new BackupDatabaseRequest
// {
//     SourcePath = "/var/lib/postgresql/data/db_prod",
//
//     Data = new byte[1024 * 1024 * 10],
//
//     OriginalSize   = 1024L * 1024 * 10,
//     CompressedSize = 0,
//     IsCompressed   = false,
//     IsEncrypted    = false,
//
//     Checksum = Guid.NewGuid().ToString("N"),
//
//     Timestamp = DateTime.Now
// };
//
//
//
// var chain = new Chain<BackupDatabaseRequest>()
//     .AddHandler(collectionDecorate)
//     .AddHandler(uploadChain)
//     .AddHandler(verifyDecorate);
//
// chain.Execute(request);
