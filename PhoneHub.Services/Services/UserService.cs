using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Exceptions;
using PhoneHub.Core.Interfaces;
using PhoneHub.Services.Interfaces;
using System.Net;

namespace PhoneHub.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;

        public UserService(IUnitOfWork unitOfWork, IPasswordService passwordService)
        {
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            return await _unitOfWork.UserRepository.GetAllDapperAsync();
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            return await _unitOfWork.UserRepository.GetByIdDapperAsync(id);
        }

        public async Task<User?> GetByCredentialsAsync(UserLogin userLogin)
        {
            var hashedPassword = _passwordService.Hash(userLogin.Password);
            var user = await _unitOfWork.UserRepository.GetByCredentialsAsync(userLogin.Email, hashedPassword);

            if (user == null)
                return null;

            // RN-07: usuario inactivo no puede iniciar sesión
            if (!user.IsActive)
                throw new BusinessException("Usuario inactivo, contacte al administrador.", HttpStatusCode.Forbidden);

            return user;
        }

        public async Task CreateUserAsync(CreateUserDto dto)
        {
            // RN-04: email único
            var existing = await _unitOfWork.UserRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new BusinessException("El email ya está registrado.", HttpStatusCode.BadRequest);

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = _passwordService.Hash(dto.Password),
                Telephone = dto.Telephone,
                Role = dto.Role,
                IsActive = true
            };

            await _unitOfWork.UserRepository.Add(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(int id, CreateUserDto dto)
        {
            var user = await _unitOfWork.UserRepository.GetById(id);
            if (user == null)
                throw new NotFoundException("Usuario no encontrado.");

            // RN-04: email único si cambió
            if (!string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existing = await _unitOfWork.UserRepository.GetByEmailAsync(dto.Email);
                if (existing != null)
                    throw new BusinessException("El email ya está registrado.", HttpStatusCode.BadRequest);
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.Password = _passwordService.Hash(dto.Password);
            user.Telephone = dto.Telephone;
            user.Role = dto.Role;

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateUserAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetById(id);
            if (user == null)
                throw new NotFoundException("Usuario no encontrado.");

            user.IsActive = false;
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateUserAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetById(id);
            if (user == null)
                throw new NotFoundException("Usuario no encontrado.");

            if (user.IsActive)
                throw new BusinessException("El usuario ya se encuentra activo.", HttpStatusCode.BadRequest);

            user.IsActive = true;
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
