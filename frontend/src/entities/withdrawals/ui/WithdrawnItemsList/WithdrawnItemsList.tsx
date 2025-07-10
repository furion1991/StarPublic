import Image from 'next/image'
import { WithdrawItem } from '../WithdrawItem/WithdrawItem'
import classes from './WithdrawnItemsList.module.scss'

export const WithdrawnItemsList = () => {
  const withdrawnItems = [
    { id: 0, status: 'success' as const },
    { id: 1, status: 'success' as const },
    { id: 2, status: 'in-process' as const },
    { id: 3, status: 'success' as const },
    { id: 4, status: 'in-process' as const },
    { id: 5, status: 'success' as const },
    { id: 6, status: 'success' as const },
    { id: 7, status: 'canceled' as const },
    { id: 8, status: 'success' as const }
  ]

  return (
    <div className={classes.withdrawnItemsList}>
      <ul className={classes.list}>
        {withdrawnItems.map(({ id, status }) => {
          return (
            <li key={id}>
              <WithdrawItem status={status} />
            </li>
          )
        })}
      </ul>

      <div className={classes.infoBottom}>
        <div className={classes.infoItem}>
          <Image src='/icons/clock-circle-filled.svg' width={30} height={30} alt='Часы' />

          <p>
            После начала вывода у тебя будет <br /> несколько минут для отмены передачи
          </p>
        </div>

        <div className={classes.infoItem}>
          <Image src='/icons/dialog-gradient.svg' width={30} height={30} alt='Часы' />

          <p>
            Если что-то пошло не по плану, ты всегда <br /> можешь обратиться в нашу тех.поддержку!
          </p>
        </div>
      </div>
    </div>
  )
}
