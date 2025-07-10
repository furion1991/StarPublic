import Image from 'next/image'
import Link from 'next/link'

import { Button } from '@/shared/ui'

import classes from './DepositSuccess.module.scss'

export const DepositSuccessPage = () => {
  return (
    <div className={classes.depositSuccessPage}>
      <h1>
        <Image src='/icons/checkmark-circle-green.svg' width={52} height={52} alt='Галочка' />
        Баланс успешно пополнен
      </h1>

      <ul className={classes.features}>
        <li>
          <div className={classes.icon}>
            <Image src='/icons/crown-gradient.svg' width={57} height={57} alt='Корона' />
          </div>

          <p>
            Отлично! Теперь вперёд выбивать <span>крутой дроп</span>!
          </p>
        </li>

        <li>
          <div className={classes.icon}>
            <Image src='/icons/medal-gradient-big.svg' width={58} height={58} alt='Медаль' />
          </div>

          <p>
            Открывай кейсы! Создавай <span>контракты</span> и <span>апгрейды</span>!
          </p>
        </li>

        <li>
          <div className={classes.icon}>
            <Image src='/icons/swords.svg' width={51} height={51} alt='Мечи' />
          </div>

          <p>
            Не забывай учавствовать в <span>ивентах</span> и <span>кейсбаттлах</span>
          </p>
        </li>
      </ul>

      <div className={classes.links}>
        <Link href='/'>
          <Button>‹ На главную</Button>
        </Link>

        <Link href='/'>
          <Button color='purple'>Перейти к ивенту ›</Button>
        </Link>
      </div>
    </div>
  )
}
