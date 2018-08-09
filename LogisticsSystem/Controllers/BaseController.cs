using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lgistic.Base.UnitOfWorks;
using Logistic.Base.DataContext;
namespace LogisticsSystem.Controllers
{
    public class BaseController:Controller
    {
        Logistic.Base.DataContext.LogisticsConnection db = new Logistic.Base.DataContext.LogisticsConnection();
        private readonly UnitOfWork _unitOfWork;

        //private LogisticsConnection _userManager;

        public UnitOfWork UnitOfWork
        {
            get { return _unitOfWork ?? (new UnitOfWork()); }
        }

        public BaseController() { }

        public BaseController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

    }
}