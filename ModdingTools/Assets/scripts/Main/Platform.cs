public class Platform : bs,IOnPlayerEnter
{
    #if game
    public void OnPlayerEnter(Player pl, Trigger other, bool b)
    {
        if (b)
        {
            pl.node.SetParent(transform, true);
            pl.onPlatform = true;
        }
        else
        {
            pl.node.SetParent(null);
            pl.onPlatform = false;    
        }

    }
#endif
}