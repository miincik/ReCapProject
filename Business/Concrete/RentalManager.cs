using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    public class RentalManager : IRentalService
    {
        IRentalDal _rentalDal;

        public RentalManager(IRentalDal rentalDal)
        {
            _rentalDal = rentalDal;
        }

        public IResult Add(Rental rental)
        {
            var result = CheckRental(rental);
            if (!result.Success)
            {
                return new ErrorResult();
            }
            _rentalDal.Add(rental);
            return new SuccessResult();
        }

        public IResult Delete(Rental rental)
        {
            var result = RentalReturnTime(rental);
            if (!result.Success)
            {
                return new ErrorResult();
            }
            _rentalDal.Delete(rental);
            return new SuccessResult();
        }

        public IDataResult<List<Rental>> GetAllByCarId(int carId)
        {
            return new SuccessDataResult<List<Rental>>(_rentalDal.GetAll(r => r.CarId == carId));
        }

        public IDataResult<List<Rental>> GetAll()
        {
            return new SuccessDataResult<List<Rental>>(_rentalDal.GetAll());
        }

        public IDataResult<Rental> GetById(int rentalId)
        {
            return new SuccessDataResult<Rental>(_rentalDal.Get(r => r.CarId == rentalId));
        }

        public IResult Update(Rental rental)
        {
            var result = RentalReturnTime(rental);
            if (!result.Success)
            {
                return new ErrorResult();
            }
            _rentalDal.Update(rental);
            return new SuccessResult();
        }

        public IResult CheckRental(Rental rental)
        {
            var result = _rentalDal.GetAll(r => r.CarId == rental.CarId);
            if (result == null)//bu id'deki araba hiç kiralanmamış//
            {
                _rentalDal.Add(rental);
                return new SuccessResult();
            }
            else
            {
                foreach (var item in result)//araba teslim alınmamışsa return date null
                {
                    if (item.ReturnDate == null)//kiralanamaz
                    {
                        return new ErrorResult();
                    }
                }
            }
            _rentalDal.Add(rental);
            return new SuccessResult();
        }

        public IResult RentalReturnTime(Rental rental)//araba teslim//return date girilir
        {
            var result = _rentalDal.GetAll(r => r.CarId == rental.CarId);//kiralancak arabanın id sinde olan rental bilgisini getir
            if (result == null)//sistemde öyle bir bilgi yok
            {
                return new ErrorResult();
            }
            else//id ile eşleşene tüm bilgiler geldi//bunlardan returnDate null olan teslim alınmamış
            {
                foreach (var item in result)//araba teslim alınmamışsa return date null
                {
                    if (item.ReturnDate == null)//teslim al
                    {
                        _rentalDal.Update(rental);
                        return new SuccessResult();
                    }
                }
                return new ErrorResult();
            }
        }
    }
}
