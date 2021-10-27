using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDataManager
{
    public static StateDatas stateDatas = new StateDatas();
    public static string nextSceneName;
    //public static List<List<Color>> playersColorList = new List<List<Color>>();
    //public static List<PlayerInput> playerInputs = new List<PlayerInput>();
    public static List<PlayerData> playerDatas = new List<PlayerData>();
    public static int lairTotalNumber;
    public static int lairCurrentNumber;
}

[System.Serializable]
public struct EnemyData
{
    public int id;
    public string name;
    public float dropProbability;
    public float maxHealth;
    public float FireRate;
    public float BulletSpeed;
    public float attackDamage;
    public float moveSpeed;
    public float inAttackRangeMoveSpeed;
    public float aiRadius;
    [Tooltip("Yellow Sphere")] [Range(0f, 500f)] public float roamRange;
    [Tooltip("White Sphere")] [Range(0f, 500f)] public float detectShipRange;
    [Tooltip("Blue Sphere")] [Range(0f, 500f)] public float ClosestDistanceToShip;
}

[System.Serializable]
public struct EnemyLairData
{
    public int id;
    public string name;
    public float maxHealth;
    public float SpawnRate;
    [Tooltip("White Sphere")] [Range(0f, 500f)] public float detectShipRange;
}

[System.Serializable]
public struct BulletData
{
    public int id;
    public string name;
    public float startSpeed;
    public float damage;
    public float chasingDelay;
    public float chasingSpeed;
    public string targetTag;
    public LayerMask targetLayer;
}

[System.Serializable]
public struct ShooterData
{
    public bool isGunHorizontal;
    public Transform gunPivotPoint;
    public Animator gunAnimator;
    public float gunMaxRotationRange;
    public float gunMovingDegreePerSec;
    public string bulletTag;
    public float bulletDamage;
    public float bulletSpeed;
    public float fireRate;
    public Transform bulletPool;
}

[System.Serializable]
public struct BoosterData
{
    public float shipSpeed;
    public float shipBoostSpeed;
    public float shipBoostModeDuration;
    public float boostersRotateSpeed;
    public Transform[] boosters;
}

[System.Serializable]
public struct LaserData
{
    public bool isGunHorizontal;
    public GameObject laserPrefab;
    public Transform gunPivotPoint;
    public Animator gunAnimator;
    public float gunMaxRotationRange;
    public float gunMovingDegreePerSec;
    public float laserChargeSpeedMulti;
    public float laserConsumeSpeedMulti;

}

[System.Serializable]
public struct SheildData
{
    public GameObject shieldSprite;
    public float maxShieldHP;
}

[System.Serializable]
public struct ShipData
{
    public int id;
    public string name;
    public int wrenchNumber;
    public int upgradeTimes;
    public float maxHealth;
    [HideInInspector] public Vector3 shipPosition;
    public int[] ShipPartLevel;
}


[System.Serializable]
public struct StateInfo
{
    public string Comment;
    public int State_id;
    public GameObject State;
}

[System.Serializable]
public class LevelInfo
{
    public string Comment;
    public int Level_id;
    public GameObject Level;
    public StateInfo[] stateInfo;
}

public class StateDatas
{
    public int Current_State_id = 0;
    public int Current_Level_id = 0;

    public List<List<int>> LevelAndStateHistory = new List<List<int>>();

}

public struct GameSettings
{
    public bool Fullscreen;
    public int ResolutionIndex;
    public int QualityIndex;
    public float MusicVolume;
    public float SoundEffectVolume;
}

[System.Serializable]
public struct Point2
{
    public int x;
    public int y;

    public Point2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public struct PlayerData
{
    public List<Color> colors;
    public int deviceId;
    public string deviceName;

    public PlayerData(List<Color> colors, int deviceId, string deviceName)
    {
        this.colors = new List<Color>();
        this.colors = colors;
        this.deviceId = deviceId;
        this.deviceName = deviceName;
    }
}

[System.Serializable]
public class GameSaveData
{
    public string LevelName;
    public ShipData shipData;
    public SheildData sheildData;
    public bool[] LairIsDead;

}