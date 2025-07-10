'use client'

import cn from 'classnames'
import { useEffect, useRef, useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'

import { Button, SparksAnimated } from '@/shared/ui'
import {
  UfoItemHolder,
  UpgradeCentralInstallation,
  type UpgradeLootItem
} from '@/entities/upgrades'

import { useAuth, useAuthModal, useShakingAnimation, useUser } from '@/shared/hooks'
import { SellItemButton } from '@/features/inventory'
import {
  getUpgradeInfo as getUpgradeInfoAPI,
  createUpgrade as createUpgradeAPI
} from '../../api/upgrades'

import classes from './UpgradeDevice.module.scss'

type UpgradeState = 'default' | 'lose' | 'win'

type AnimationControls = {
  isAnimationInProcess: boolean
  isShaking: boolean
  isGlowing: boolean
  isExplosionVisible: boolean
  isSparksActive: boolean
  startAnimation: () => void
}

type UpgradeDeviceProps = {
  animationControls: AnimationControls
  userItem: UpgradeLootItem | null
  itemToUpgrade: UpgradeLootItem | null
}

export const UpgradeDevice = ({
  userItem,
  itemToUpgrade,
  animationControls
}: UpgradeDeviceProps) => {
  const queryClient = useQueryClient()
  const { isAuth } = useAuth()
  const { user } = useUser()
  const { openAuthModal } = useAuthModal()

  const [upgradeState, setUpgradeState] = useState<UpgradeState>('default')

  const userItemRef = useRef<HTMLDivElement>(null)
  const centralInstallationRef = useRef<HTMLDivElement>(null)
  const wantedItemRef = useRef<HTMLDivElement>(null)
  const {
    isAnimationInProcess,
    isExplosionVisible,
    isShaking,
    isGlowing,
    isSparksActive,
    startAnimation
  } = animationControls

  useShakingAnimation({
    elementsRef: [userItemRef, centralInstallationRef, wantedItemRef],
    enabled: isShaking
  })

  const { data: upgradeInfo, mutate: getUpgradeInfo } = useMutation({
    mutationFn: getUpgradeInfoAPI
  })

  const {
    data: upgradeResult,
    mutate: createUpgrade,
    isPending: isUpgradeCreating
  } = useMutation({
    mutationFn: createUpgradeAPI,
    onSuccess: () => {
      // update upgrades history & inventory items
      queryClient.invalidateQueries({
        queryKey: ['me']
      })

      startAnimation()
    }
  })

  // win / lose
  useEffect(() => {
    if (typeof upgradeResult !== 'undefined' && !isAnimationInProcess) {
      const isUpgradeItemsIdsMatch = upgradeResult?.items[0].item.id === itemToUpgrade?.itemId

      setUpgradeState(isUpgradeItemsIdsMatch ? 'win' : 'lose')
    }
  }, [upgradeResult, isAnimationInProcess])

  useEffect(() => {
    if (!userItem || !itemToUpgrade) return

    setUpgradeState('default')

    getUpgradeInfo({
      userItemId: userItem.itemId,
      attemptedItemId: itemToUpgrade.itemId
    })
  }, [userItem, itemToUpgrade])

  const handleUpgrade = () => {
    if (!user || !userItem?.inventoryId || !itemToUpgrade) return

    setUpgradeState('default')

    createUpgrade({
      userId: user.id,
      inventoryItemId: userItem.inventoryId,
      attemptedItemId: itemToUpgrade.itemId
    })
  }

  const getResultUfoColor = (state: UpgradeState) => {
    switch (state) {
      case 'default':
        return 'default'
      case 'lose':
        return 'red'
      case 'win':
        return 'green'
    }
  }

  return (
    <div className={classes.upgradeDevice}>
      <div ref={userItemRef} className={cn(classes.itemLeft)}>
        <UfoItemHolder
          direction='left'
          item={userItem}
          color={getResultUfoColor(upgradeState)}
          glowing={isGlowing}
        />
      </div>

      <div ref={centralInstallationRef} className={cn(classes.main)}>
        <UpgradeCentralInstallation
          state={upgradeState}
          explosion={isExplosionVisible}
          glowing={isGlowing}
        />

        <SparksAnimated active={isSparksActive} />

        <div className={classes.upgradeInfo}>
          <div className={classes.indicator}>
            <p>{upgradeInfo ? upgradeInfo.chance : '0.00'}%</p>
            <p>Шанс апгрейда</p>
          </div>

          <div className={classes.indicator}>
            <p>{upgradeInfo ? upgradeInfo.coefficient : '0.00'}x</p>
            <p>Коэффициент</p>
          </div>
        </div>

        <div className={cn(classes.action, classes.needAuth)}>
          {!isAnimationInProcess ? (
            isAuth ? (
              userItem && itemToUpgrade ? (
                <Button
                  boxShadow
                  loading={isUpgradeCreating}
                  className={classes.upgradeBtn}
                  onClick={handleUpgrade}
                >
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
            )
          ) : null}
        </div>
      </div>

      <div ref={wantedItemRef} className={cn(classes.itemRight)}>
        <UfoItemHolder
          direction='right'
          item={itemToUpgrade}
          color={getResultUfoColor(upgradeState)}
          glowing={isGlowing}
          ItemSellSlot={
            upgradeResult?.items[0] && upgradeState === 'win' ? (
              <SellItemButton
                className={classes.sellItemBtn}
                inventoryItemId={upgradeResult?.items[0].inventoryRecordId}
                sellPrice={upgradeResult?.items[0].item.sellPrice}
              />
            ) : null
          }
        />
      </div>
    </div>
  )
}
