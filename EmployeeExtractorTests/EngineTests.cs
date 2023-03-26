namespace EmployeeExtractorTests
{
    using System.Reflection;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    using Moq;
    using NUnit.Framework.Internal;

    using EmployeeExtractor.Services;

    public class EngineTests
    {
        private Engine _engine;
        private string _resourcesPath;

        [SetUp]
        public void Setup()
        {
            var engineLoggerMock = new Mock<ILogger<Engine>>();
            var fileParserLoggerMock = new Mock<ILogger<FileParser>>();

            var fileParse = new FileParser(fileParserLoggerMock.Object);
            _engine = new Engine(engineLoggerMock.Object, fileParse);

            var assemblyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            _resourcesPath = Path.Combine(assemblyPath, "TestingFiles");   
        }

        [Test]
        public void ParseCsvCustomModel()
        {
            var fileWithoutHeaders = Path.Combine(_resourcesPath, "FileWithoutHeaders.csv");

            using var stream = new FileStream(fileWithoutHeaders, FileMode.Open);
            var formFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(fileWithoutHeaders))
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            var result = _engine.FileParser.ParseCsvCustomModelAsync(formFile);

            Assert.AreEqual(14, result.Result.Count);
        }

        [Test]
        public void CalculateWorkerPairs()
        {
            var fileWithoutHeaders = Path.Combine(_resourcesPath, "FileWithoutHeaders.csv");

            using var stream = new FileStream(fileWithoutHeaders, FileMode.Open);
            var formFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(fileWithoutHeaders))
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            var parsedCollection = _engine.FileParser.ParseCsvCustomModelAsync(formFile);
            var result = _engine.CalculateWorkerPairs(parsedCollection.GetAwaiter().GetResult());

            Assert.AreEqual(2, result.CsvWorkerDublicatesCollection.Count);
            Assert.AreEqual(1, result.CsvWorkedCollection.Count);
        }
    }
}