﻿using PuertoRico.Engine.Domain.Misc;
using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.DTOs
{
    public class CargoShipDto
    {
        public GoodType? GoodType { get; set; }
        public int Capacity { get; set; }
        public int GoodCount { get; set; }

        public CargoShipDto(CargoShip cargoShip) {
            GoodType = cargoShip.GoodType;
            Capacity = cargoShip.Capacity;
            GoodCount = cargoShip.GoodCount;
        }
    }
}