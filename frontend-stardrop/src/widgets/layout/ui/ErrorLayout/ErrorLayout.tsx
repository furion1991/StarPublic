import Image from 'next/image'

import { Button } from '@/shared/ui'

import classes from './ErrorLayout.module.scss'

export const ErrorLayout = () => {
  return (
    <div className={classes.wrapper}>
      <Image src='/img/chest.png' width={525} height={325} quality={100} alt='Сундук' />

      <h1>
        Произошла <br /> неизвестная ошибка
      </h1>

      <Button
        boxShadow
        onClick={() => {
          window.location.reload()
        }}
      >
        Перезагрузить страницу
      </Button>
    </div>
  )
}
