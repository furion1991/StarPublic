import Image from 'next/image'
import classes from './CurrentPointsNumber.module.scss'
import Link from 'next/link'

export const CurrentPointsNumber = () => {
  return (
    <div className={classes.currentPointsNumber}>
      <Image src='/icons/coin.svg' width={42} height={45} alt='Монета' />

      <div className={classes.text}>
        <p>20</p>
        <p>До 100 места</p>
      </div>

      <Link href='/'>Условия</Link>
    </div>
  )
}
