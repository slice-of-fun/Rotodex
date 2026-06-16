namespace Roto.Core;

public interface IRestrictVersion
{
    bool CanBeReceivedByVersion(GameVersion version);
}
