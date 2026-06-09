using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.DataTest
{
    [TestClass]
    public class LoggerUnitTest
    {
        [TestMethod]
        public void LoggerWriteTestMethod()
        {
            Logger newInstance = new Logger();

            newInstance.ClearLog();

            DateTime date = DateTime.Now;

            newInstance.Log("Test log message", date);

            string logEntry = $"{date}: Test log message";

            string logFilePath = newInstance.GetLogFilePath();

            Assert.IsNotNull(logFilePath);

            Thread.Sleep(newInstance.GetWriteInterval() * 2);

            string lastLine = File.ReadLines(logFilePath).LastOrDefault();

            Assert.AreEqual(logEntry, lastLine);

        }

        [TestMethod]
        public void LoggerWriteIntervalIsRespectedTestMethod()
        {
            // Sprawdza że wpisy NIE trafiają do pliku natychmiast po Log(),
            // tylko dopiero po upływie interwału timera
            Logger newInstance = new Logger();
            newInstance.ClearLog();

            DateTime date = DateTime.Now;
            newInstance.Log("delayed message", date);

            // Czytamy plik PRZED upływem interwału 
            string logFilePath = newInstance.GetLogFilePath();
            string[] linesBefore = File.ReadAllLines(logFilePath);
            Assert.IsFalse(
                linesBefore.Any(line => line.Contains("delayed message")),
                "Wpis nie powinien być w pliku przed upływem interwału timera");

            // Teraz czekamy — wpis powinien się pojawić
            Thread.Sleep(newInstance.GetWriteInterval() * 2);

            string[] linesAfter = File.ReadAllLines(logFilePath);
            Assert.IsTrue(
                linesAfter.Any(line => line.Contains("delayed message")),
                "Wpis powinien być w pliku po upływie interwału timera");
        }

        [TestMethod]
        public void LoggerFlushesQueueOnDisposeTestMethod()
        {
            Logger newInstance = new Logger();
            newInstance.ClearLog();

            string message = "message before dispose";
            DateTime date = DateTime.Now;
            newInstance.Log(message, date);

            newInstance.Dispose();

            string logFilePath = newInstance.GetLogFilePath();
            string[] allLines = File.ReadAllLines(logFilePath);

            Assert.IsTrue(
                allLines.Any(line => line.Contains("message before dispose")),
                "Dispose() powinien zapisać do pliku wpisy które jeszcze czekały w kolejce");
        }
    }
}
