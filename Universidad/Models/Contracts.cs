namespace Universidad.Models
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}