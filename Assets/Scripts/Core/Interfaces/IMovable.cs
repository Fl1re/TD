using System.Threading;
using Cysharp.Threading.Tasks;

public interface IMovable
{
    UniTask MoveAsync(CancellationToken ct);
}