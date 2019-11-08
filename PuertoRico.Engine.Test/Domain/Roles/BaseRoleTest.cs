using System;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Domain.Roles;
using Xunit;

namespace PuertoRico.Engine.Test.Domain.Roles
{
    public abstract class BaseRoleTest<T> where T : IRole
    {
        protected readonly IRole _role;
        protected readonly Game _game;
        protected readonly IPlayer _roleOwner;
        protected readonly int _doubloonsOnRole;

        public BaseRoleTest() {
            _doubloonsOnRole = 3;
            _game = new Game(3);
            _role = (T)Activator.CreateInstance(typeof(T), _game);
            for (var i = 0; i < _doubloonsOnRole; i++) {
                _role.AddOneDoubloon();
            }
            _roleOwner = _game.CurrentPlayer;
            _roleOwner.SelectRole(_role);
        }

        [Fact]
        public void StackedDoubloonsMovedToPlayer() {
            Assert.Equal(0, _role.StackedDoubloons);
            Assert.Equal(_doubloonsOnRole, _roleOwner.Doubloons);
        }

    }
}