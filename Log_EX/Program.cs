using System;
using System.Diagnostics;


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
            string strNowTime = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"); // 파일 형식 년 - 월 - 일 - 시 - 분 - 초

            if (Directory.Exists("log") == false)
                Directory.CreateDirectory("log"); // 폴더가 없을경우 폴더 생성

            fileStream = new FileStream($"log\\log.{strNowTime}.csv", FileMode.CreateNew); // 파일 생성
            streamWrite = new StreamWriter(fileStream, System.Text.Encoding.UTF8); // 한글로 인코딩
        }

        public void Log(ELogLv eLogLv, string desc)
        {
            // eLogLv이 info이면 -> info, warn, error 로그 남김
            // eLogLv이 warn이면 -> warn, error 로그 남김
            // eLogLv이 error이면 -> error 로그 남김
            if (eCurLogLv < eLogLv)
                return;

            StackFrame stackFrame = new StackFrame(1, true);
            string funcName = stackFrame.GetMethod().Name; //프레임이 실행 되고 있는 메서드 가져오기
            string fileName = stackFrame.GetFileName(); // 실행 중인 코드를 포함하는 파일 이름 가져오기 (실행 파일의 디버깅 기호에서 추출)

            string fileLine = stackFrame.GetFileLineNumber().ToString(); // 실행중인 코드를 포함하는 파일에서 줄 번호 가져오기 ( 실행파일의 디버깅 기호에서 추출).스택 추적 생성

            // ,(콤마)는 .csv 파일에서 구분문자로 사용
            desc = desc.Replace(",", ".");

            desc = $"{eLogLv.ToString()},\t{desc},\t{funcName},\t{fileName} {fileLine},\t{DateTime.Now}";

            streamWrite.WriteLine(desc);
            streamWrite.Flush();// 이 스트림의 버퍼를 지우고 버퍼링된 모든 데이터가 파일에 쓰여지도록 함
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
