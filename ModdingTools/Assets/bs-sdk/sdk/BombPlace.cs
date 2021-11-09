
public class BombPlace : ItemBase
{
    #if game
    public TextMeshOnScreen txt;
    public override void Start()
    {
        base.Start();

        txt = Instantiate(_Game.c4TextPrefab,transform);
        txt.SetActive(false);
    }
    public void Update()
    {
        txt.pos = transform.position;
    }
    protected override void OnCreate(bool b)
    {
        base.OnCreate(b);
        Register(this,b);
    }
    #endif

}