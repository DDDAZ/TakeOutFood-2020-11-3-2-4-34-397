using System.Linq;

namespace TakeOutFood
{
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

            // get item string, total price before promotion, item lists for promotion. 
            var itemLine = inputs.Select(GetItemLine).ToList();
            var total = itemLine.Sum(item => double.Parse(item.Split(' ')[^2]));
            var itemsPromotion = inputs.Select(GetPromotionName).ToList().Where(i => i != "").ToList();

            return itemsPromotion.Count != 0 ?
                PromotionUsed(itemsPromotion, itemLine, total) : PromotionUnused(itemLine, total);
        }

        private string PromotionUnused(List<string> itemLine, double total)
        {
            return "============= Order details =============\n" +
                   string.Join("", itemLine) +
                   "-----------------------------------\n" +
                   $"Total：{total:F0} yuan\n" +
                   "===================================";
        }

        private string PromotionUsed(List<string> itemsPromotion, List<string> itemLine, double total)
        {
            var itemsHavePromotion = string.Join(", ", itemsPromotion);
            var costSaved = itemLine.Where(item => itemsHavePromotion.Contains(item.Split(' ')[0]))
                .Sum(item => double.Parse(item.Split(' ')[^2]) / 2);

            return "============= Order details =============\n" +
                   string.Join("", itemLine) +
                   "-----------------------------------\n" +
                   "Promotion used:\n" +
                   $"Half price for certain dishes ({itemsHavePromotion}), saving {costSaved:F0} yuan\n" +
                   "-----------------------------------\n" +
                   $"Total：{(total - costSaved):F0} yuan\n" +
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

            return promotions.Contains(id) ? itemRepository.FindAll().First(x => x.Id == id).Name : "";
        }
    }
}
