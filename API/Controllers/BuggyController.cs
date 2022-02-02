using API.Errors;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace API.Controllers;

public class BuggyController : BaseApiController
{
    private readonly StoreContext context;

    public BuggyController(StoreContext context)
    {
        this.context = context;
    }

    [HttpGet("testauth")]
    [Authorize]
    public ActionResult<string> GetSecretText()
    {
        return "secret stuff";
    }

    [HttpGet("notfound")]
    public ActionResult GetNotFoundRequest()
    {
        Product thing = context.Products.Find(Guid.NewGuid());

        if (thing == null)
        {
            return NotFound(new ApiResponse(404));
        }

        return Ok();
    }

    [HttpGet("servererror")]
    public ActionResult GetServerError()
    {
        Product thing = context.Products.Find(Guid.NewGuid());
        string thingToReturn = thing.ToString();

        return Ok();
    }

    [HttpGet("badrequest")]
    public ActionResult GetBadRequest()
    {
        return BadRequest(new ApiResponse(400));
    }

    [HttpGet("badrequest/{id}")]
    public ActionResult GetBadRequest(Guid id)
    {
        return BadRequest();
    }
}
