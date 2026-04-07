namespace TextRPG.Models;

public class Enemy : Character
{
    #region 프로퍼티
    public int GoldReward { get; private set; }
    

    #endregion

    #region 생성자
    protected Enemy(string name, int maxHp, int maxMp, int attackPower, int defense, int level, int goldReward)
        : base(name, maxHp, maxMp, attackPower, defense, level)
    {
        GoldReward = goldReward;
    }
    #endregion

    #region 메서드
    // 적 생성 메서드 (레벨에 따른 난이도 조절)
    public static Enemy CreateEnemy(int playerLevel)
    {
        // 난수 생성기
        Random random = new Random();
        
        // 적 캐릭터의 레벨 (플레이어 레벨 +-1)
        int enemyLevel = Math.Max(1, playerLevel + random.Next(-1, 2)); // -1, 0, +1 

        // 적 캐릭터의 종류
        string[] enemyTypes = { "고블린", "오크", "트롤" };
        string enemyName = enemyTypes[random.Next(0, enemyTypes.Length)];
        
        // 적 캐릭터의 스탯 (레벨에 비례)
        int maxHp = 50 + (enemyLevel - 1) * 20;
        int maxMp = 20 + (enemyLevel - 1) * 10;
        int attackPower = 20 + (enemyLevel - 1) * 5;
        int defense = 5 + (enemyLevel - 1) * 3;
        int goldReward = 20 + (enemyLevel - 1) * 10;

        return new Enemy($"Lv{enemyLevel} {enemyName}", maxHp, maxMp, attackPower, defense, enemyLevel, goldReward);
    }

    // 적 캐릭터 정보 출력
    public override void DisplayInfo()
    {
        Console.WriteLine($"==== {Name} 정보 ====");
        Console.WriteLine($"레벨: {Level}");
        Console.WriteLine($"체력: {CurrentHP}/{MaxHP}");
        Console.WriteLine($"공격력: {AttackPower}");
        Console.WriteLine($"방어력: {Defense}");
        Console.WriteLine("=======================");
        
    }
    
    public override int Attack(Character target)
    {
        // return target.TakeDamage(AttackPower);
        
        // 랜덤 공격력 부여
        // 일반공격 (70%) / 강한공격 (30%)
        Random random = new Random();

        if (random.NextDouble() < 0.7)
        {
            // 일반공격
            return target.TakeDamage(AttackPower);
        }
        else
        {
            // 강한공격
            Console.WriteLine($"{Name}의 강한공격!");
            int damage = (int)(AttackPower * 1.5f);
            return target.TakeDamage(damage);
        }
    }

    #endregion
}