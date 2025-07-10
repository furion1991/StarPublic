import { PriceWithCurrency } from '@/shared/ui'
import classes from './PrizeWinning.module.scss'

type PrizeWinningProps = {
  prize: number
  multiplier: number
}

export const PrizeWinning = ({ prize, multiplier }: PrizeWinningProps) => {
  return (
    <div className={classes.prizeWinning}>
      <div className={classes.left}>
        <p>Вы успели забрать!</p>
        <p>x {multiplier}</p>
      </div>

      <div className={classes.prize}>
        <PriceWithCurrency>{prize}</PriceWithCurrency>

        <p>Ваш выигрыш</p>
      </div>
    </div>
  )
}
