'use client'

import Image from 'next/image'
import cn from 'classnames'
import { useState } from 'react'

import { Button, PriceWithCurrency } from '@/shared/ui'

import { useAuth, useAuthModal } from '@/shared/hooks'

import classes from './UpgradeDevice.module.scss'

type Item = {
  id: string
  name: string
  sellPrice: number
  image: string
  rarity: number
}

type UpgradeState = 'default' | 'loose' | 'win'

type UpgradeDeviceProps = {
  upgradeItem: Item | null
  itemToUpgrade: Item | null
}

export const UpgradeDevice = ({ upgradeItem, itemToUpgrade }: UpgradeDeviceProps) => {
  const { isAuth } = useAuth()
  const { openAuthModal } = useAuthModal()

  const [upgradeState, setUpgradeState] = useState<UpgradeState>('default')

  const handleUpgrade = () => {
    if (Math.random() < 0.5) {
      setUpgradeState('loose')
    } else {
      setUpgradeState('win')
    }
  }

  const getUpgradeStateImage = (state: UpgradeState) => {
    switch (state) {
      case 'default':
        return '/img/upgrades/upgrade-default-state.svg'
      case 'loose':
        return '/img/upgrades/upgrade-loose.svg'
      case 'win':
        return '/img/upgrades/upgrade-win.svg'
    }
  }

  const placeholder = (
    <div className={classes.itemPlaceholder}>
      <div className={classes.placeholderImage}>
        <Image
          src='/img/upgrades/upgrade-gun-placeholder.svg'
          width={163}
          height={143}
          alt='Пистолет'
        />
      </div>

      <p>Выберите предмет с вашего инвентаря</p>
    </div>
  )

  return (
    <div className={classes.upgradeDevice}>
      <div className={classes.itemLeft}>
        <div className={classes.ufo}>
          <Image
            src='/img/upgrades/ufo-left.png'
            width={380}
            height={422}
            quality={100}
            priority
            alt='НЛО'
          />
        </div>

        {upgradeItem ? (
          <div className={classes.itemPlaceholder}>
            <div className={classes.upgradeItemImage}>
              <Image src={upgradeItem.image} width={200} height={200} alt={upgradeItem.name} />
            </div>

            <p className={classes.itemName}>{upgradeItem.name}</p>
          </div>
        ) : (
          placeholder
        )}
      </div>

      <div className={classes.main}>
        <div className={classes.mainBg}>
          <Image
            src='/img/upgrades/upgrade-device-main-bg.png'
            width={781}
            height={847}
            quality={100}
            priority
            alt='Главный фон'
          />

          <div className={classes.stateImg}>
            <Image
              src={getUpgradeStateImage(upgradeState)}
              width={235}
              height={235}
              quality={100}
              priority
              alt='Состояние'
            />
          </div>
        </div>

        <div className={classes.upgradeInfo}>
          <div className={classes.indicator}>
            <p>0.00%</p>
            <p>Шанс апгрейда</p>
          </div>

          <div className={classes.indicator}>
            <p>0.00x</p>
            <p>Коэффициент</p>
          </div>
        </div>

        <div className={cn(classes.action, classes.needAuth)}>
          {isAuth ? (
            upgradeItem && itemToUpgrade ? (
              <Button boxShadow className={classes.upgradeBtn} onClick={handleUpgrade}>
                Апгрейд
              </Button>
            ) : null
          ) : (
            <>
              <p>Для апгрейда необходимо авторизоваться</p>

              <Button className={classes.authBtn} boxShadow onClick={openAuthModal}>
                Войти
              </Button>
            </>
          )}
        </div>
      </div>

      <div className={classes.itemRight}>
        <div className={classes.ufo}>
          <Image
            src='/img/upgrades/ufo-right.png'
            width={380}
            height={422}
            quality={100}
            priority
            alt='НЛО'
          />
        </div>

        {itemToUpgrade ? (
          <div className={classes.itemPlaceholder}>
            <div className={classes.upgradeItemImage}>
              <Image src={itemToUpgrade.image} width={200} height={200} alt={itemToUpgrade.name} />
            </div>

            <p className={classes.itemName}>{itemToUpgrade.name}</p>

            {upgradeState === 'win' ? (
              <Button className={classes.sellItemBtn} color='purple'>
                Продать за
                <PriceWithCurrency>{itemToUpgrade.sellPrice}</PriceWithCurrency>
              </Button>
            ) : null}
          </div>
        ) : (
          placeholder
        )}
      </div>
    </div>
  )
}
