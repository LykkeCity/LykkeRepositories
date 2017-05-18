using System.Threading.Tasks;

namespace Lykke.Core.Timers.Interfaces
{
	public interface ITimerCommand
	{
		Task Execute();
	}
}