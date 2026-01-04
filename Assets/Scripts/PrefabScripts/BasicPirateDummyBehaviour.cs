
public class BasicPirateDummyBehaviour : EnemyTemplate
{
    protected override void Awake()
    {
        thisThrusterSet = Thrusters.GunPodPirateThrusterSet;
        Health = 100;
        rotationDegreesPerSeconds = 50;
        base.Awake();
        Fuel = 2000;
        MaxAccel = 2000;
        FuelUsage = 1;

    }
}