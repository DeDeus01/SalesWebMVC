using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesWebMVC.Services;
using SalesWebMVC.Models;
using SalesWebMVC.Models.ViewModels;
using SalesWebMVC.Services.Exceptions;

namespace SalesWebMVC.Controllers
{
    public class SellersController : Controller
    {
        private readonly SellerService _sellerService;
        private readonly DepartmentService _departmentService;

        public SellersController(SellerService sellerService, DepartmentService departmentService) //estancia a dependencia para a classe serviço.
        {
            _sellerService = sellerService;
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _sellerService.FindAllAsync(); //o controlador manda o serviço chamar o metodo find all e trazer a lista.

            return View(list); //retorna um oobjeto IActionResult -> view na pasta Sellers
        }


        public async Task<IActionResult> Create()
        {
            var departments = await _departmentService.FindAllAsync(); //ao abrir a pagina ja traz  a lista do banco
            var viewModel = new SellerFormViewModel { Departmens = departments };
            return View(viewModel); //passa pra view a lista de departamentos na view.
        }

        [HttpPost] // serve pra indicar que o metodo é uma ação de post. 
        [ValidateAntiForgeryToken] //serve para previnir a app de ataque csrf usando a sessao ativa.
        public async Task<IActionResult> Create(Seller seller)
        {
            if (!ModelState.IsValid) //serve para aumentar a validação dos dados antes de salvar, caso o javascript nao funcione.
            {
                return View(seller);
            }

            await _sellerService.InsertAsync(seller);
            return RedirectToAction(nameof(Index)); // o metodo retorna um redirecionamento para o metodo index 
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error),new { message = "Id not provided"});
            }

            var obj = await _sellerService.FindByIdAsync(id.Value);

            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not found na base" });
            }

            return View(obj);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not provided" }); ;
            }

            var obj = await _sellerService.FindByIdAsync(id.Value);

            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not found" }); ;
            }

            List<Department> departments = await _departmentService.FindAllAsync();
            SellerFormViewModel viewModel = new SellerFormViewModel { Seller = obj, Departmens = departments };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Seller seller)
        {
            if (!ModelState.IsValid) //validação no servidor, quando uma requisição vai com erros ele exibe as mensagens de validação.
            {
                var departments = await _departmentService.FindAllAsync();
                var viewModel = new SellerFormViewModel { Seller = seller, Departmens = departments };
                return View(viewModel);
            }
            if (id != seller.Id) //id diferente do objeto é errado
            {
                return RedirectToAction(nameof(Error), new { message = "Id mismatch" }); ;
            }
            try
            {
                await _sellerService.UpdateAsync(seller);
                return RedirectToAction(nameof(Index)); //redireciona para o metodo index desta classe.(tela index)

            } catch (NotFoundException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message }); ;
            }
            catch (DbConcurrencyException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message }); ;
            }
        }

        //Metodo get que busca o objeto a retorna a view com o objeto.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not provided" }); ;
            }

            var obj = await _sellerService.FindByIdAsync(id.Value); //pesquisa o objeto usando o serviço.

            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not found" }); ;
            }

            return View(obj);
        }

        [HttpPost]              //O metodo executou corretamente pois a solicitação foi POST. analisar melhor.
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _sellerService.RemoveAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message});
            }
        }

        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View(viewModel);
        }
    }
}