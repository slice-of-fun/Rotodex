using RotoDex.Adapter;

namespace RotoDex.Core.Commands
{
    public class EditPokemonCommand : ICommand
    {
        private readonly ISaveFile _saveFile;
        private readonly int _boxIndex;
        private readonly int _slotIndex;
        private readonly IPokemon _oldState;
        private readonly IPokemon _newState;

        public EditPokemonCommand(ISaveFile saveFile, int boxIndex, int slotIndex, IPokemon oldState, IPokemon newState)
        {
            _saveFile = saveFile;
            _boxIndex = boxIndex;
            _slotIndex = slotIndex;
            _oldState = oldState;
            _newState = newState;
        }

        public void Execute()
        {
            // Apply new state
            _saveFile.SetBoxPokemon(_boxIndex, _slotIndex, _newState.Clone());
        }

        public void Undo()
        {
            // Revert to old state
            _saveFile.SetBoxPokemon(_boxIndex, _slotIndex, _oldState.Clone());
        }
    }
}
