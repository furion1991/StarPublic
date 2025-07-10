using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DtoClassLibrary.DataTransferObjects.Bonus
{
    public class DiscountBonusDto : BonusBaseDto, IBonusDto
    {
        public decimal DiscountPercentage { get; set; }
    }
}
