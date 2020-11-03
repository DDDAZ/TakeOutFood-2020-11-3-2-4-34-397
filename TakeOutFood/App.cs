using System.Dynamic;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace TakeOutFood
{
    using System;
    using System.Collections.Generic;

    public class App
    {
        private IItemRepository itemRepository;
        private ISalesPromotionRepository salesPromotionRepository;

        public App(IItemRepository itemRepository, ISalesPromotionRepository salesPromotionRepository)
        {
            this.itemRepository = itemRepository;
            this.salesPromotionRepository = salesPromotionRepository;
        }

        public string BestCharge(List<string> inputs)
        {
            //TODO: write code here
            var itemLine = inputs.Select(GetItemLine).ToList();
            var itemsPromotion = inputs.Select(GetPromotionName).ToList();
            for (int i = 0; i < itemsPromotion.Count; i++)
            {
                if (itemsPromotion[i] == null)
                {
                    itemsPromotion.RemoveAt(i);
                }
            }
            var total = itemLine.Sum(item => double.Parse(item.Split(' ')[^2]));

            if (itemsPromotion.Count != 0)
            {
                var itemsHavePromotion = string.Join(", ", itemsPromotion);
                double costSaved = 0;

                foreach (var item in itemLine)
                {
                    if (itemsHavePromotion.Contains(item.Split(' ')[0]))
                    {
                        costSaved += double.Parse(item.Split(' ')[^2]) / 2;
                    }
                }
                return "============= Order details =============\n" +
                       string.Join("", itemLine) +
                       "-----------------------------------\n" +
                       "Promotion used:\n" +
                       $"Half price for certain dishes ({itemsHavePromotion}), saving {costSaved:F0} yuan\n" +
                       "-----------------------------------\n" +
                       $"Total：{(total - costSaved):F0} yuan\n" +
                       "===================================";
            }

            return "============= Order details =============\n" +
                   string.Join("", itemLine) +
                   "-----------------------------------\n" +
                   $"Total：{total:F0} yuan\n" +
                   "===================================";
        }

        private string GetItemLine(string item)
        {
            var id = item.Split(' ')[0];
            var count = item.Split(' ')[2];
            var itemFind = itemRepository.FindAll().First(x => x.Id == id);
            // "Braised chicken x 1 = 18 yuan\n"
            return $"{itemFind.Name} x {count} = {double.Parse(count) * itemFind.Price:F0} yuan\n";
        }

        private string GetPromotionName(string item)
        {
            var id = item.Split(' ')[0];
            var promotions = salesPromotionRepository.FindAll()[0].RelatedItems;

            return promotions.Contains(id) ? itemRepository.FindAll().First(x => x.Id == id).Name : null;
        }
    }
}
