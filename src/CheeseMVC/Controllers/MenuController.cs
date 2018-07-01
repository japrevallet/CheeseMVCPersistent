using CheeseMVC.Data;
using CheeseMVC.Models;
using CheeseMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CheeseMVC.Controllers
{
    public class MenuController : Controller
    {
        private CheeseDbContext context;

        public MenuController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            IList<Menu> menus = context.Menus.ToList();
            return View(menus);
        }

        [HttpGet]
        public IActionResult Add()
        {
            AddMenuViewModel addMenuViewModel = new AddMenuViewModel();
            return View(addMenuViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel addCheeseMenuModel)
        {
            if (ModelState.IsValid)
            {
                Menu newMenu = new Menu
                {
                    Name = addCheeseMenuModel.Name
                };
                context.Menus.Add(newMenu);
                context.SaveChanges();
                return Redirect("/Menu/ViewMenu/" + newMenu.ID);
            }
            return View(addCheeseMenuModel);
        }
        
        [HttpGet]
        public IActionResult ViewMenu(int id)
        {
            Menu menu = context.Menus.Single(m => m.ID == id);

            List<CheeseMenu> items = context
                    .CheeseMenus
                    .Include(item => item.Cheese)
                    .Where(cm => cm.MenuID == id)
                    .ToList();

            ViewMenuViewModel viewMenuViewModel = new ViewMenuViewModel
            {
                Menu = menu,
                Items = items
            };

            return View(viewMenuViewModel);
        }


        [HttpGet]
        public IActionResult AddItem(int ID, string msg)
        {
            Menu Menu = context.Menus.Single(m => m.ID == ID);
            List<Cheese> Cheeses = context.Cheeses.ToList();

            AddMenuItemViewModel addMenuItemViewModel = new AddMenuItemViewModel(Menu, Cheeses);
            return View(addMenuItemViewModel);

        }

        [HttpPost]
        public IActionResult AddItem(AddMenuItemViewModel addMenuItemViewModel)
        {
            if (ModelState.IsValid)
            {
                var cheeseID = addMenuItemViewModel.cheeseID;
                var menuID = addMenuItemViewModel.menuID;

                IList<CheeseMenu> existingItems = context.CheeseMenus
                        .Where(cm => cm.CheeseID == cheeseID)
                        .Where(cm => cm.MenuID == menuID).ToList();
                if (existingItems.Count == 0)
                {
                    CheeseMenu menuItem = new CheeseMenu
                    {
                        Cheese = context.Cheeses.Single(c => c.ID == cheeseID),
                        Menu = context.Menus.Single(m => m.ID == menuID)
                    };
                    context.CheeseMenus.Add(menuItem);
                    context.SaveChanges();
                }
                return Redirect(string.Format("/Menu/ViewMenu/{0}", addMenuItemViewModel.menuID));
            }
            return View(addMenuItemViewModel);
        }

    }
}
