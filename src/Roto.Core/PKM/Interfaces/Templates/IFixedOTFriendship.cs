namespace Roto.Core;

/// <summary>
/// Exposes a friendship value with the original trainer.
/// </summary>
public interface IFixedOTFriendship
{
    byte OriginalTrainerFriendship { get; }
}
