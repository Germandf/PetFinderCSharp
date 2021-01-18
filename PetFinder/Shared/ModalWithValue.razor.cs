using Microsoft.AspNetCore.Components;

namespace PetFinder.Shared
{
    public partial class ModalWithValue<TypeValue>
    {
        [Parameter] public TypeValue value { get; set; }

        public void valueChange(TypeValue args)
        {
            value = args;
        }
    }
}