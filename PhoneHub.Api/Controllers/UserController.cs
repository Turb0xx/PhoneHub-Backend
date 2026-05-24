using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneHub.Api.Responses;
using PhoneHub.Core.DTOs;
using PhoneHub.Core.Enum;
using PhoneHub.Services.Interfaces;
using PhoneHub.Services.Validators;
using System.Net;

namespace PhoneHub.Api.Controllers
{
    [Authorize(Roles = nameof(RoleType.Admin))]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly CreateUserDtoValidator _validator;

        public UserController(
            IUserService userService,
            CreateUserDtoValidator validator)
        {
            _userService = userService;
            _validator = validator;
        }

        /// <summary>
        /// Obtiene la lista de todos los usuarios registrados (CU-06). Solo Admin.
        /// </summary>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<UserDto>>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            var response = new ApiResponse<IEnumerable<UserDto>>(users);
            return Ok(response);
        }

        /// <summary>
        /// Obtiene el detalle de un usuario por su ID (CU-06). Solo Admin.
        /// </summary>
        /// <param name="id">ID del usuario.</param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<UserDto>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound("Usuario no encontrado.");

            var response = new ApiResponse<UserDto>(user);
            return Ok(response);
        }

        /// <summary>
        /// Crea un nuevo usuario en el sistema (CU-02).
        /// RN-04: el email debe ser único.
        /// </summary>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await _userService.CreateUserAsync(dto);
            var response = new ApiResponse<string>("Usuario creado correctamente.");
            return Ok(response);
        }

        /// <summary>
        /// Actualiza los datos de un usuario existente (CU-02). Solo Admin.
        /// </summary>
        /// <param name="id">ID del usuario a actualizar.</param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUserDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await _userService.UpdateUserAsync(id, dto);
            var response = new ApiResponse<string>("Usuario actualizado correctamente.");
            return Ok(response);
        }

        /// <summary>
        /// Reactiva un usuario previamente desactivado. Solo accesible para Admin.
        /// </summary>
        /// <param name="id">ID del usuario a reactivar.</param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [HttpPatch("{id}/activar")]
        public async Task<IActionResult> Activate(int id)
        {
            await _userService.ActivateUserAsync(id);
            var response = new ApiResponse<string>($"Usuario con ID {id} reactivado correctamente. Ya puede iniciar sesión.");
            return Ok(response);
        }

        /// <summary>
        /// Desactiva un usuario (RN-06). No se elimina físicamente: cambia IsActive a false.
        /// Solo accesible para Admin.
        /// </summary>
        /// <param name="id">ID del usuario a desactivar.</param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _userService.DeactivateUserAsync(id);
            var response = new ApiResponse<string>($"Usuario con ID {id} desactivado correctamente. Ya no podrá iniciar sesión.");
            return Ok(response);
        }
    }
}
