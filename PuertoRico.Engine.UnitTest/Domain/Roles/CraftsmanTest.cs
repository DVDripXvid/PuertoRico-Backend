using System;
using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Buildings;
using PuertoRico.Engine.Domain.Buildings.Production.Large;
using PuertoRico.Engine.Domain.Buildings.Production.Small;
using PuertoRico.Engine.Domain.Buildings.Small;
using PuertoRico.Engine.Domain.Resources;
using PuertoRico.Engine.Domain.Resources.Goods;
using PuertoRico.Engine.Domain.Roles;
using PuertoRico.Engine.Domain.Tiles.Plantations;
using PuertoRico.Engine.Extensions;
using Xunit;

namespace PuertoRico.Engine.UnitTest.Domain.Roles
{
    public class CraftsmanTest : BaseRoleTest<Craftsman>
    {
        [Fact]
        public void CanUsePrivilege() {
            var action = new BonusProduction {
                GoodType = GoodType.Corn
            };
            var plantation = new CornPlantation();
            plantation.AddWorker(new Colonist());
            RoleOwner.Plant(plantation);

            ReselectRole();
            CanExecuteActionOnce(action, RoleOwner);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void ProductionTheory(ProductionTestCase testCase) {
            testCase.Definitions.ForEach(def => {
                var player = Game.Players[def.PlayerIndex];
                player.Doubloons = 0;
                def.Buildings.ForEach(bDef => {
                    var building = (IBuilding) Activator.CreateInstance(bDef.BuildingType);
                    for (var i = 0; i < bDef.WorkerCount; i++) {
                        building.AddWorker(new Colonist());
                    }

                    player.Build(building);
                });
                def.Plantations.ForEach(pDef => {
                    var plantation = (IPlantation) Activator.CreateInstance(pDef.PlantationType);
                    if (pDef.HasWorker) {
                        plantation.AddWorker(new Colonist());
                    }

                    player.Plant(plantation);
                });
            });

            ReselectRole();

            testCase.Definitions.ForEach(def => {
                var player = Game.Players[def.PlayerIndex];
                foreach (GoodType goodType in Enum.GetValues(typeof(GoodType))) {
                    if (def.ExpectedProduction.ContainsKey(goodType)) {
                        Assert.Equal(def.ExpectedProduction[goodType], player.Goods.OfGoodType(goodType).Count());
                    }
                    Assert.Equal(def.ExpectedDoubloon, player.Doubloons);
                }
            });
        }
        
        public static IEnumerable<object[]> TestData => new[] {
            new[] {
                new ProductionTestCase {
                    Definitions = new List<PlayerProductionDefinition> {
                        new PlayerProductionDefinition {
                            PlayerIndex = 0,
                            Buildings = new List<BuildingDefinition> {
                                new BuildingDefinition {
                                    BuildingType = typeof(SmallSugarMill),
                                    WorkerCount = 1
                                },
                            },
                            Plantations = new List<PlantationDefinition> {
                                new PlantationDefinition {
                                    PlantationType = typeof(SugarPlantation),
                                    HasWorker = true
                                },
                            },
                            ExpectedProduction = new Dictionary<GoodType, int> {
                                {GoodType.Sugar, 1}
                            }
                        },
                        new PlayerProductionDefinition {
                            PlayerIndex = 1,
                            Buildings = new List<BuildingDefinition> {
                                new BuildingDefinition {
                                    BuildingType = typeof(SugarMill),
                                    WorkerCount = 2
                                },
                            },
                            Plantations = new List<PlantationDefinition> {
                                new PlantationDefinition {
                                    PlantationType = typeof(CornPlantation),
                                    HasWorker = true
                                },
                            },
                            ExpectedProduction = new Dictionary<GoodType, int> {
                                {GoodType.Corn, 1}
                            }
                        },
                        new PlayerProductionDefinition {
                            PlayerIndex = 2,
                            Buildings = new List<BuildingDefinition> {
                                new BuildingDefinition {
                                    BuildingType = typeof(SmallSugarMill),
                                    WorkerCount = 1
                                },
                            },
                            Plantations = new List<PlantationDefinition> {
                                new PlantationDefinition {
                                    PlantationType = typeof(SugarPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(SugarPlantation),
                                    HasWorker = true
                                },
                            },
                            ExpectedProduction = new Dictionary<GoodType, int> {
                                {GoodType.Sugar, 1}
                            }
                        }
                    }
                },
            },
            new[] {
                new ProductionTestCase {
                    Definitions = new List<PlayerProductionDefinition> {
                        new PlayerProductionDefinition {
                            PlayerIndex = 0,
                            Buildings = new List<BuildingDefinition>(),
                            Plantations = new List<PlantationDefinition> {
                                new PlantationDefinition {
                                    PlantationType = typeof(CornPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(CornPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(CornPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(CornPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(CornPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(CornPlantation),
                                    HasWorker = true
                                },
                            },
                            ExpectedProduction = new Dictionary<GoodType, int> {
                                {GoodType.Corn, 6}
                            }
                        },
                        new PlayerProductionDefinition {
                            PlayerIndex = 1,
                            Buildings = new List<BuildingDefinition>(),
                            Plantations = new List<PlantationDefinition> {
                                new PlantationDefinition {
                                    PlantationType = typeof(CornPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(CornPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(CornPlantation),
                                    HasWorker = true
                                },
                            },
                            ExpectedProduction = new Dictionary<GoodType, int> {
                                {GoodType.Corn, 3}
                            }
                        },
                        new PlayerProductionDefinition {
                            PlayerIndex = 2,
                            Buildings = new List<BuildingDefinition>(),
                            Plantations = new List<PlantationDefinition> {
                                new PlantationDefinition {
                                    PlantationType = typeof(CornPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(CornPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(CornPlantation),
                                    HasWorker = true
                                },
                            },
                            ExpectedProduction = new Dictionary<GoodType, int> {
                                {GoodType.Corn, 1}
                            }
                        }
                    }
                },
            },
            new[] {
                new ProductionTestCase {
                    Definitions = new List<PlayerProductionDefinition> {
                        new PlayerProductionDefinition {
                            PlayerIndex = 0,
                            Buildings = new List<BuildingDefinition> {
                                new BuildingDefinition {
                                    BuildingType = typeof(CoffeeRoaster),
                                    WorkerCount = 2,
                                },
                                new BuildingDefinition {
                                    BuildingType = typeof(TobaccoStorage),
                                    WorkerCount = 3,
                                }
                            },
                            Plantations = new List<PlantationDefinition> {
                                new PlantationDefinition {
                                    PlantationType = typeof(CoffeePlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(CoffeePlantation),
                                    HasWorker = false
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(TobaccoPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(TobaccoPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(TobaccoPlantation),
                                    HasWorker = true
                                },
                            },
                            ExpectedProduction = new Dictionary<GoodType, int> {
                                {GoodType.Coffee, 1},
                                {GoodType.Tobacco, 3}
                            }
                        },
                        new PlayerProductionDefinition {
                            PlayerIndex = 1,
                            Buildings = new List<BuildingDefinition> {
                                new BuildingDefinition {
                                    BuildingType = typeof(IndigoPlant),
                                    WorkerCount = 1,
                                },
                                new BuildingDefinition {
                                    BuildingType = typeof(SmallIndigoPlant),
                                    WorkerCount = 1,
                                },
                                new BuildingDefinition {
                                    BuildingType = typeof(TobaccoStorage),
                                    WorkerCount = 3,
                                }
                            },
                            Plantations = new List<PlantationDefinition> {
                                new PlantationDefinition {
                                    PlantationType = typeof(IndigoPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(IndigoPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(TobaccoPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(TobaccoPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(TobaccoPlantation),
                                    HasWorker = true
                                },
                            },
                            ExpectedProduction = new Dictionary<GoodType, int> {
                                {GoodType.Indigo, 2},
                                {GoodType.Tobacco, 3}
                            }
                        },
                        new PlayerProductionDefinition {
                            PlayerIndex = 2,
                            Buildings = new List<BuildingDefinition> {
                                new BuildingDefinition {
                                    BuildingType = typeof(SugarMill),
                                    WorkerCount = 1,
                                },
                                new BuildingDefinition {
                                    BuildingType = typeof(SmallSugarMill),
                                    WorkerCount = 1,
                                },
                                new BuildingDefinition {
                                    BuildingType = typeof(TobaccoStorage),
                                    WorkerCount = 3,
                                }
                            },
                            Plantations = new List<PlantationDefinition> {
                                new PlantationDefinition {
                                    PlantationType = typeof(SugarPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(SugarPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(TobaccoPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(TobaccoPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(TobaccoPlantation),
                                    HasWorker = true
                                },
                            },
                            ExpectedProduction = new Dictionary<GoodType, int> {
                                {GoodType.Sugar, 2},
                                {GoodType.Tobacco, 3}
                            }
                        }
                    }
                },
            },
            new[] {
                new ProductionTestCase {
                    Definitions = new List<PlayerProductionDefinition> {
                        new PlayerProductionDefinition {
                            PlayerIndex = 0,
                            Buildings = new List<BuildingDefinition> {
                                new BuildingDefinition {
                                    BuildingType = typeof(Factory),
                                    WorkerCount = 1,
                                },
                            },
                            Plantations = new List<PlantationDefinition> {
                                new PlantationDefinition {
                                    PlantationType = typeof(CornPlantation),
                                    HasWorker = true,
                                },
                            },
                            ExpectedProduction = new Dictionary<GoodType, int> {
                                {GoodType.Corn, 1}
                            },
                            ExpectedDoubloon = 0,
                        },
                        new PlayerProductionDefinition {
                            PlayerIndex = 1,
                            Buildings = new List<BuildingDefinition> {
                                new BuildingDefinition {
                                    BuildingType = typeof(SugarMill),
                                    WorkerCount = 1
                                },
                                new BuildingDefinition {
                                    BuildingType = typeof(TobaccoStorage),
                                    WorkerCount = 1
                                },
                                new BuildingDefinition {
                                    BuildingType = typeof(Factory),
                                    WorkerCount = 1,
                                },
                            },
                            Plantations = new List<PlantationDefinition> {
                                new PlantationDefinition {
                                    PlantationType = typeof(SugarPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(TobaccoPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(CornPlantation),
                                    HasWorker = true
                                },
                            },
                            ExpectedProduction = new Dictionary<GoodType, int> {
                                {GoodType.Corn, 1},
                                {GoodType.Sugar, 1},
                                {GoodType.Tobacco, 1}
                            },
                            ExpectedDoubloon = 2,
                        },
                        new PlayerProductionDefinition {
                            PlayerIndex = 2,
                            Buildings = new List<BuildingDefinition> {
                                new BuildingDefinition {
                                    BuildingType = typeof(Factory),
                                    WorkerCount = 1,
                                },
                                new BuildingDefinition {
                                    BuildingType = typeof(SmallSugarMill),
                                    WorkerCount = 1
                                },
                                new BuildingDefinition {
                                    BuildingType = typeof(SmallIndigoPlant),
                                    WorkerCount = 1
                                },
                                new BuildingDefinition {
                                    BuildingType = typeof(TobaccoStorage),
                                    WorkerCount = 1
                                },
                                new BuildingDefinition {
                                    BuildingType = typeof(CoffeeRoaster),
                                    WorkerCount = 1
                                },
                            },
                            Plantations = new List<PlantationDefinition> {
                                new PlantationDefinition {
                                    PlantationType = typeof(SugarPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(IndigoPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(TobaccoPlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(CoffeePlantation),
                                    HasWorker = true
                                },
                                new PlantationDefinition {
                                    PlantationType = typeof(CornPlantation),
                                    HasWorker = true
                                },
                            },
                            ExpectedProduction = new Dictionary<GoodType, int> {
                                {GoodType.Sugar, 1},
                                {GoodType.Indigo, 1},
                                {GoodType.Tobacco, 1},
                                {GoodType.Coffee, 1},
                                {GoodType.Corn, 1},
                            },
                            ExpectedDoubloon = 5,
                        }
                    }
                },
            },
        };
    }

    public class ProductionTestCase
    {
        public List<PlayerProductionDefinition> Definitions { get; set; }
    }

    public class PlayerProductionDefinition
    {
        public int PlayerIndex { get; set; }
        public List<BuildingDefinition> Buildings { get; set; }
        public List<PlantationDefinition> Plantations { get; set; }
        public Dictionary<GoodType, int> ExpectedProduction { get; set; }
        public int ExpectedDoubloon { get; set; } = 0;
    }

    public class BuildingDefinition
    {
        public Type BuildingType { get; set; }
        public int WorkerCount { get; set; }
    }

    public class PlantationDefinition
    {
        public Type PlantationType { get; set; }
        public bool HasWorker { get; set; }
    }
}