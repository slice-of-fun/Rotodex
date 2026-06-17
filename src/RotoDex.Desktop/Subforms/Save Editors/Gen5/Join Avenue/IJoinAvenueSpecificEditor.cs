using Roto.Core;

namespace RotoDex.Desktop;

internal interface IJoinAvenueSpecificEditor<in T> where T : class, IJoinAvenueEntity5
{
    void LoadObject(T entity);
    void SaveObject(T entity);
}
