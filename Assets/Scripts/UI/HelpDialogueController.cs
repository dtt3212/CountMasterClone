namespace CountMasterClone
{
    public class HelpDialogueController : DialogueController
    {
        protected override void Awake()
        {
            base.Awake();

            Content = @"Move and go through gates to increase the number of clone on your team! Avoid enemies and hostiles.
The more clones you have, the more coins you will get, which will allow you to purchase custom skin and sound effects!";
        }
    }
}