'use client'

import { useEffect, useState } from 'react'
import Image from 'next/image'
import cn from 'classnames'
import { FormProvider, useForm } from 'react-hook-form'

import { BonusesList, Task } from '@/entities/bonus-wheel'
import { Button, TextField, Timer } from '@/shared/ui'
import { useAuthModal, useAuth } from '@/shared/hooks'

import classes from './BonusWheel.module.scss'

type WheelState = 'can-spin' | 'timeout' | 'promocode' | 'drop' | 'need-auth'

export const BonusWheel = () => {
  const [wheelState, setWheelState] = useState<WheelState>('need-auth')

  const { isAuth } = useAuth()
  const { openAuthModal } = useAuthModal()
  const useFormProps = useForm()
  const { handleSubmit, reset } = useFormProps

  useEffect(() => {
    if (!isAuth) {
      setWheelState('need-auth')
    } else {
      setWheelState('can-spin')
    }
  }, [isAuth])

  return (
    <div className={classes.bonusWheel}>
      {wheelState !== 'drop' ? (
        <div className={classes.bonusesList}>
          <BonusesList />
        </div>
      ) : null}

      <div className={classes.content}>
        {wheelState === 'can-spin' ? (
          <div className={cn(classes.wheelStage, classes.canSpin)}>
            <Image
              src='/img/bonus-wheel/bonus-wheel-text.png'
              quality={100}
              width={358}
              height={174}
              alt='Барабан бонусов'
            />

            <Button
              boxShadow
              onClick={() => {
                setWheelState('drop')
              }}
            >
              Крутить барабан
            </Button>
          </div>
        ) : null}

        {wheelState === 'timeout' ? (
          <div className={cn(classes.wheelStage, classes.timeout)}>
            <Image
              src='/img/bonus-wheel/bonus-wheel-text.png'
              quality={100}
              width={358}
              height={174}
              alt='Барабан бонусов'
            />

            <div className={classes.timer}>
              <Timer expiryTime='15 jul 2026 18:00:00' styleVariant={2} />
            </div>

            <Button
              boxShadow
              onClick={() => {
                setWheelState('promocode')
              }}
            >
              Ввести код
            </Button>
          </div>
        ) : null}

        {wheelState === 'drop' ? (
          <div className={cn(classes.wheelStage, classes.drop)}>
            <div className={classes.droppedBonus}>
              <Image
                src='/placeholders/bonus-wheel-loot.png'
                width={224}
                height={236}
                alt='Бонус'
              />
            </div>

            <div className={classes.dropActions}>
              <Button
                onClick={() => {
                  setWheelState('can-spin')
                }}
              >
                Крутить еще
              </Button>

              <Button
                color='purple'
                onClick={() => {
                  setWheelState('timeout')
                }}
              >
                Использовать
              </Button>
            </div>

            <div className={classes.tasksContainer}>
              <p>Или можете выполнить задание</p>

              <ul className={classes.tasksList}>
                <li>
                  <Task taskText='Вам необходимо сделать определёное действие' isDone />
                </li>

                <li>
                  <Task taskText='Вам необходимо сделать определёное действие' />
                </li>
              </ul>
            </div>
          </div>
        ) : null}

        {wheelState === 'need-auth' ? (
          <div className={cn(classes.wheelStage, classes.needAuth)}>
            <Image
              src='/img/bonus-wheel/need-auth-text.png'
              quality={100}
              width={223}
              height={148}
              alt='Нужно войти'
            />

            <div className={classes.needAuthText}>
              <p>Для того, что бы испытать свою удачу, необходимо авторизоваться</p>
            </div>

            <Button onClick={openAuthModal}>Войти</Button>
          </div>
        ) : null}

        {wheelState === 'promocode' ? (
          <div className={cn(classes.wheelStage, classes.promocode)}>
            <Image
              src='/img/bonus-wheel/enter-promocode-text.png'
              quality={100}
              width={324}
              height={155}
              alt='Введите промокод'
            />

            <FormProvider {...useFormProps}>
              <form
                className={classes.promocodeForm}
                onSubmit={handleSubmit(() => {
                  reset()
                  setWheelState('can-spin')
                })}
              >
                <TextField
                  className={classes.promocodeField}
                  name='promocode'
                  placeholder='Введите промокод'
                />

                <Button
                  type='submit'
                  boxShadow
                  onClick={() => {
                    setWheelState('promocode')
                  }}
                >
                  Применить
                </Button>
              </form>
            </FormProvider>
          </div>
        ) : null}
      </div>
    </div>
  )
}
