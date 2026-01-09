using DesignPattern.BehaviourPattern.Cor.Solution;

// CoR
var collectChain = new DatabaseCollectionHandle();
var uploadChain  = new CloudUpHandle();
var verifyChain  = new VerificationHandler();

// Decorate
Handler<BackupDatabaseRequest> collectionDecorate = collectChain;
collectionDecorate = new CompressionDecorator(collectionDecorate);
collectionDecorate = new EncryptionDecoration(collectionDecorate);

Handler<BackupDatabaseRequest> verifyDecorate = verifyChain;
verifyDecorate = new PerformanceMonitoringDecorator(verifyDecorate);

var request = new BackupDatabaseRequest
{
    SourcePath = "/var/lib/postgresql/data/db_prod",

    Data = new byte[1024 * 1024 * 10],

    OriginalSize   = 1024L * 1024 * 10,
    CompressedSize = 0,
    IsCompressed   = false,
    IsEncrypted    = false,

    Checksum = Guid.NewGuid().ToString("N"),

    Timestamp = DateTime.Now
};



var chain = new Chain<BackupDatabaseRequest>()
    .AddHandler(collectionDecorate)
    .AddHandler(uploadChain)
    .AddHandler(verifyDecorate);

chain.Execute(request);
    