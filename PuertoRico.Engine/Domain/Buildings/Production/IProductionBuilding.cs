using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Domain.Buildings.Production
{
    public interface IProductionBuilding : IBuilding
    {
        bool CanProduce<TGood>() where TGood : IGood;
    }
}