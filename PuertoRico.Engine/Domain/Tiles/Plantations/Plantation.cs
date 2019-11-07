using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Domain.Tiles.Plantations
{
    public abstract class Plantation<TGood> : Tile, IPlantation where TGood : IGood
    {
        public bool CanProduce<T>() where T : IGood {
            return typeof(TGood) == typeof(T);
        }
    }
}