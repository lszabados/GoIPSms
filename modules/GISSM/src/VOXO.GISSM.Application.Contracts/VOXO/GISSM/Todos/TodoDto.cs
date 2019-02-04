using System;
using Volo.Abp.Application.Dtos;

namespace VOXO.GISSM.Todos
{
    public class TodoDto : EntityDto<Guid>
    {
        public string Text { get; set; }
    }
}