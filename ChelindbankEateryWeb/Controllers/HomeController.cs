using System.Diagnostics;
using ChelindbankEateryWeb.Domain.Document;
using Microsoft.AspNetCore.Mvc;
using ChelindbankEateryWeb.Models;

namespace ChelindbankEateryWeb.Controllers;

public class HomeController : Controller
{
	private readonly DocumentHolder _documentHolder;

	public HomeController(DocumentHolder documentHolder)
	{
		_documentHolder = documentHolder;
	}

	public async Task<IActionResult> Index()
	{
		var document = await _documentHolder.GetInfo();
		if (document == null || !document.IsActual())
			return View("MenuDoesntExist");
		return View(document);
	}

	public async Task<DateTime> LatestUpdateDate()
	{
		var document = await _documentHolder.GetInfo();
		if (document == null)
			return DateTime.MinValue;
		return document.DocumentUpdated;
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error() { return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); }
}
