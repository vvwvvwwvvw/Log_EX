using System;
using static Log_EX.LogManager;

namespace Log_EX
{
    public enum ELogLv
    {
        info = 2,
        warn = 1,
        error = 0,
    }

    public class LogManager
    {
        FileStream fileStream = null;
        StreamWriter streamWrite = null;
        public ELogLv eCurLogLv = ELogLv.info;

        public LogManager()
        {
            CreateLogFile();
        }

        public void CreateLogFile()
        {
            string strNowTime = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

            if (Directory.Exists("log") == false)
                Directory.CreateDirectory("log");

            fileStream = new FileStream($"log\\log.{strNowTime}.txt", FileMode.CreateNew);
            streamWrite = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
        }

        public void Log(ELogLv eLogLv, string desc)
        {
            // eLogLv이 info이면 -> info, warn, error 로그 남김
            // eLogLv이 warn이면 -> warn, error 로그 남김
            // eLogLv이 error이면 -> error 로그 남김
            if (eCurLogLv < eLogLv)
                return;

            System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame(1, true);
            string funcName = stackFrame.GetMethod().Name;
            string fileName = stackFrame.GetFileName();
            string fileLine = stackFrame.GetFileLineNumber().ToString();

            // ,(콤마)는 .csv 파일에서 구분문자로 사용
            desc = desc.Replace(",", ".");

            desc = $"{eLogLv.ToString()},\t{desc},\t{funcName},\t{fileName} {fileLine},\t{DateTime.Now}";

            streamWrite.WriteLine(desc);
            streamWrite.Flush();
            fileStream.Flush();

            Int64 fileSize = fileStream.Length;

            // 파일 사이즈가 2메가가 넘으면 해당 파일 닫은 후 파일 생성
            if (fileSize > 2097152)
            {
                streamWrite.Close();
                fileStream.Close();
                CreateLogFile();
            }
        }
    }

    public class Play
    {
        LogManager log;

        public Play(LogManager log)
        {
            this.log = log;
        }

        public void LogTest()
        {
            log.Log(ELogLv.info, "로그 내용 입력");
        }
    }
    class Program
    {

        static void Main(string[] args)
        {
            LogManager logManager = new LogManager();
            Play play = new Play(logManager);
            play.LogTest();
        }
    }
}
