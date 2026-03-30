using CustomerOrderSystem.DTOs.OrderItems;
using CustomerOrderSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CustomerOrderSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderItemsController(IOrderItemService orderItemService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<OrderItemResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<OrderItemResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var orderItems = await orderItemService.GetAllAsync(cancellationToken);
        return Ok(orderItems);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderItemResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderItemResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var orderItem = await orderItemService.GetByIdAsync(id, cancellationToken);
        return Ok(orderItem);
    }

    [HttpGet("order/{orderId:int}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<OrderItemResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<OrderItemResponseDto>>> GetByOrderId(int orderId, CancellationToken cancellationToken)
    {
        var orderItems = await orderItemService.GetByOrderIdAsync(orderId, cancellationToken);
        return Ok(orderItems);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderItemResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderItemResponseDto>> Create([FromBody] CreateOrderItemRequestDto request, CancellationToken cancellationToken)
    {
        var createdOrderItem = await orderItemService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdOrderItem.Id }, createdOrderItem);
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderItemRequestDto request, CancellationToken cancellationToken)
    {
        await orderItemService.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await orderItemService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

