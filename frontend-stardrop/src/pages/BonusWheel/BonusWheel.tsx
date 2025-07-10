import Image from 'next/image'

import { BonusWheel } from '@/widgets/bonus-wheel'
import { BonusItem, SuperBonusWord } from '@/entities/bonus-wheel'
import { PageActions } from '@/shared/ui'

import classes from './BonusWheel.module.scss'

export const BonusWheelPage = () => {
  return (
    <>
      <PageActions />

      <div className={classes.bonusWheel}>
        <div className={classes.headerBg}>
          <Image
            src='/img/bonus-wheel/bonus-wheel-header-bg.png'
            priority
            quality={100}
            fill
            style={{
              objectFit: 'cover'
            }}
            alt='Фон'
          />
        </div>

        <div className={classes.content}>
          <div className={classes.bonusWheel}>
            <BonusWheel />
          </div>

          <div className={classes.superBonus}>
            <div className={classes.superBonusHeading}>
              <h2>Супербонус</h2>

              <p>
                Соберите слово <span>Stardrop</span>
              </p>
            </div>

            <div className={classes.superBonusWord}>
              <SuperBonusWord word='stardrop' activatedLetters={['s', 'a']} />
            </div>

            <p className={classes.bonusDescription}>
              Новый бонус будет заменять предыдущий, если тот не был использован ранее. <br />{' '}
              Бонусы выпавшие из колеса, не суммируются с промокодами для пополнения баланса.
            </p>
          </div>

          <ul className={classes.bonusesList}>
            {Array.from({ length: 18 })
              .fill(null)
              .map((_, idx) => {
                return <BonusItem key={idx} />
              })}
          </ul>
        </div>
      </div>
    </>
  )
}
