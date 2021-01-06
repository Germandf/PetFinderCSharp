using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Shared
{
    public partial class ModalWithValue<TypeValue>
    {
        [Parameter]
        public TypeValue value { get; set; }
        public void valueChange(TypeValue args) => value = args;
    }
}
