import Image from 'next/image'
import Link from 'next/link'

import { Button } from '@/shared/ui'

import classes from './NotFound.module.scss'

export const NotFoundPage = () => {
  return (
    <div className={classes.wrapper}>
      <Image src='/img/chest.png' width={525} height={325} quality={100} alt='Сундук' />

      <h1>
        Такой страницы <br /> не существует
      </h1>

      <Link href='/'>
        <Button boxShadow>Вернуться на главную</Button>
      </Link>
    </div>
  )
}
