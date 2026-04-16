using TextRPG.Utils;
using TextRPG.Data;
using TextRPG.Systems;

namespace TextRPG;

class Program
{
    static void Main(string[] args)
    {
        // 콘솔 인코딩 설정(한글 지원)
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        // 저장된 게임 존재 여부 확인
        if (SaveLoadSystem.IsSaveFileExist())
        {
            ShowStartMenu();
        }
        else
        {
            GameManager.Instance.StartGame();    
        }
    }

    static void ShowStartMenu()
    {
        Console.Clear();
        ConsoleUI.ShowTitle();
        Console.WriteLine("\n╔════════════════════════════════╗");
        Console.WriteLine("║       게임 시작                ║");
        Console.WriteLine("╚════════════════════════════════╝\n"); 
        
        Console.WriteLine("\n1. 새 게임");
        Console.WriteLine("2. 이어서 하기");
        Console.WriteLine("0. 종료");

        while (true)
        {
            Console.Write("\n선택: ");
            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    // 새 게임 시작
                    GameManager.Instance.StartGame();
                    break;
                case "2":
                    // 이어서 하기
                    if (GameManager.Instance.LoadGame())
                    {
                        GameManager.Instance.StartGame(true);
                    }
                    return;
                case "0":
                    Console.WriteLine("\n게임을 종료합니다.");
                    Environment.Exit(0);
                    return;
                default:
                    Console.WriteLine("잘못된 입력입니다. 다시 선택해주세요.");
                    break;
            }
        }
    }
}