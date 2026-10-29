namespace CarDealership.Models
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}