using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Domain.Tiles.Plantations
{
    public interface IPlantation : ITile
    {
        bool CanProduce<T>() where T : IGood;
    }
}