using System;
using UTM.Keto.Application.BLogic;
using UTM.Keto.Application.Interfaces;

namespace UTM.Keto.Application
{
    public class BusinessLogicFactory
    {
        private static BusinessLogicFactory _instance;
        private readonly Lazy<IUserBL> _userBL;
        private readonly Lazy<IRoleBL> _roleBL;
        private readonly Lazy<IProductBL> _productBL;
        private readonly Lazy<ICartBL> _cartBL;
        private readonly Lazy<IOrderBL> _orderBL;

        private BusinessLogicFactory()
        {
            _userBL = new Lazy<IUserBL>(() => new UserBL());
            _roleBL = new Lazy<IRoleBL>(() => new RoleBL());
            _productBL = new Lazy<IProductBL>(() => new ProductBL());
            _cartBL = new Lazy<ICartBL>(() => new CartBL());
            _orderBL = new Lazy<IOrderBL>(() => new OrderBL());
        }

        public static BusinessLogicFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BusinessLogicFactory();
                }
                return _instance;
            }
        }

        public IUserBL GetUserBL() => _userBL.Value;
        public IRoleBL GetRoleBL() => _roleBL.Value;
        public IProductBL GetProductBL() => _productBL.Value;
        public ICartBL GetCartBL() => _cartBL.Value;
        public IOrderBL GetOrderBL() => _orderBL.Value;

        public ISupportBL GetSupportBL()
        {
            return new SupportBL();
        }

        public IReviewBL GetReviewBL()
        {
            return new ReviewBL();
        }

        public IFeedbackBL GetFeedbackBL()
        {
            return new FeedbackBL();
        }
    }
} 