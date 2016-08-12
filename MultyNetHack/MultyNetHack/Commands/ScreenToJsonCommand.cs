
namespace MultyNetHack.Commands
{
    class ScreenToJsonCommand : BaseCommand
    {
        public string FileName;
        public ScreenToJsonCommand(string FileName)
        {
            this.FileName = FileName;
        }
    }
}
