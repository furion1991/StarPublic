'use client'

import Image from 'next/image'

import { Button, PageActions } from '@/shared/ui'
import { BonusesFAQ } from '@/entities/bonuses'
import { BonusesList } from '@/widgets/bonuses'

import { useAuth, useAuthModal } from '@/shared/hooks'

import classes from './Bonuses.module.scss'

export const BonusesPage = () => {
  const { isAuth } = useAuth()
  const { openAuthModal } = useAuthModal()

  return (
    <>
      <PageActions />

      <div className={classes.bonusesPage}>
        <div className={classes.wrapper}>
          <h1>Бонусы StarDrop</h1>

          {!isAuth ? (
            <div className={classes.needAuth}>
              <Image
                src='/icons/exclamation-mark.svg'
                width={56}
                height={56}
                alt='Восклицательный знак'
              />

              <div className={classes.needAuthText}>
                <p>Для участия необходимо авторизоваться</p>
                <p>Доступ к бонусам предоставляется только авторизованым пользователям</p>
              </div>

              <Button onClick={openAuthModal}>Авторизация ›</Button>
            </div>
          ) : null}

          <BonusesList />

          <section className={classes.howItWorks}>
            <h2>
              <Image src='/icons/info-rounded-blue.svg' width={34} height={34} alt='инфо' />

              <span>Как это работает?</span>
            </h2>

            <div className={classes.howItWorksAccordionList}>
              <BonusesFAQ />
            </div>
          </section>
        </div>
      </div>
    </>
  )
}
